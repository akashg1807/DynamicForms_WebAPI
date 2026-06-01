import React, { useState, useEffect } from 'react';

const DynamicForm = ({ token, formId }) => {
  const [fields, setFields] = useState([]);
  const [formData, setFormData] = useState({});
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(true);

  // 1. Fetch form structure metadata based on the logged-in user's role
  useEffect(() => {
    const fetchFormStructure = async () => {
      try {
        const response = await fetch(`https://localhost:7143/api/form/${formId}`, {
          method: 'GET',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });

        if (!response.ok) throw new Error('Failed to retrieve form permissions.');
        const data = await response.json();
        setFields(data);

        // Pre-populate empty fields state layout
        const initialState = {};
        data.forEach(field => {
          initialState[field.fieldName] = field.fieldType === 'Checkbox' ? false : '';
        });
        setFormData(initialState);
      } catch (err) {
        setMessage(`Error: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchFormStructure();
  }, [formId, token]);

  // 2. Handle standard input state updates dynamically
  const handleChange = (fieldName, value) => {
    setFormData(prev => ({
      ...prev,
      [fieldName]: value
    }));
  };

  // 3. Dispatch data stringified to the backend storage endpoint
  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage('');

    try {
      const response = await fetch('https://localhost:7143/api/form/submit', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          formId: formId,
          jsonData: JSON.stringify(formData) // Flatten answer keys directly to string JSON
        })
      });

      if (!response.ok) throw new Error('Submission rejected by server boundary.');
      const data = await response.json();
      setMessage(`🎉 ${data.message}`);
    } catch (err) {
      setMessage(`Submission Error: ${err.message}`);
    }
  };

  if (loading) return <div className="text-center py-4 text-gray-500">Loading authorized layout keys...</div>;

  return (
    <div className="space-y-6">
      {message && (
        <div className="p-3 rounded text-sm text-center font-medium bg-green-50 text-green-700 border border-green-200">
          {message}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        {fields.map((field) => {
          // Parse string rules securely if they exist
          const rules = field.validationRules ? JSON.parse(field.validationRules) : {};
          const isReadOnly = field.permissionLevel === 'Read';

          return (
            <div key={field.fieldId} className="border-b pb-4 last:border-none">
              <label className="block text-sm font-semibold text-gray-700 mb-1">
                {field.fieldName}
                {field.isRequired && <span className="text-red-500 ml-1">*</span>}
                {isReadOnly && <span className="ml-2 text-xs text-amber-600 bg-amber-50 px-2 py-0.5 rounded font-normal">Read-Only</span>}
              </label>

              {/* RENDER BOX BY INPUT TYPE METADATA TYPE */}
              {field.fieldType === 'Checkbox' ? (
                <input
                  type="checkbox"
                  disabled={isReadOnly}
                  className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500 disabled:opacity-50"
                  checked={!!formData[field.fieldName]}
                  onChange={(e) => handleChange(field.fieldName, e.target.checked)}
                />
              ) : field.fieldType === 'Number' ? (
                <input
                  type="number"
                  required={field.isRequired}
                  disabled={isReadOnly}
                  min={rules.min}
                  max={rules.max}
                  className="mt-1 block w-full border border-gray-300 rounded p-2 focus:ring-blue-500 focus:border-blue-500 bg-gray-50 disabled:bg-gray-100 disabled:text-gray-500 disabled:cursor-not-allowed"
                  value={formData[field.fieldName] || ''}
                  onChange={(e) => handleChange(field.fieldName, e.target.value)}
                />
              ) : field.fieldType === 'Date' ? (
                <input
                  type="date"
                  required={field.isRequired}
                  disabled={isReadOnly}
                  className="mt-1 block w-full border border-gray-300 rounded p-2 focus:ring-blue-500 focus:border-blue-500 bg-gray-50 disabled:bg-gray-100 disabled:text-gray-500"
                  value={formData[field.fieldName] || ''}
                  onChange={(e) => handleChange(field.fieldName, e.target.value)}
                />
              ) : (
                <input
                  type="text"
                  required={field.isRequired}
                  disabled={isReadOnly}
                  minLength={rules.minLength}
                  maxLength={rules.maxLength}
                  className="mt-1 block w-full border border-gray-300 rounded p-2 focus:ring-blue-500 focus:border-blue-500 bg-gray-50 disabled:bg-gray-100 disabled:text-gray-500"
                  value={formData[field.fieldName] || ''}
                  onChange={(e) => handleChange(field.fieldName, e.target.value)}
                />
              )}
            </div>
          );
        })}

        <button
          type="submit"
          className="w-full mt-4 bg-emerald-600 text-white p-2 rounded font-semibold hover:bg-emerald-700 transition duration-200"
        >
          Submit Form Responses
        </button>
      </form>
    </div>
  );
};

export default DynamicForm;