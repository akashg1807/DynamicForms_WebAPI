import React, { useState } from 'react';
import Login from './components/Login';
import DynamicForm from './components/DynamicForm'; // 🔄 Import the form engine

function App() {
  const [authData, setAuthData] = useState(null);

  const handleLoginSuccess = (data) => {
    setAuthData(data);
  };

  const handleLogout = () => {
    setAuthData(null);
  };

  if (!authData) {
    return <Login onLoginSuccess={handleLoginSuccess} />;
  }

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-4xl mx-auto bg-white rounded shadow p-6">
        <div className="flex justify-between items-center border-b pb-4 mb-6">
          <div>
            <h1 className="text-xl font-bold text-gray-800">Enterprise Form Workspace</h1>
            <p className="text-sm text-gray-600">Logged in as: <span className="font-semibold">{authData.username}</span> (Role ID: {authData.roleId})</p>
          </div>
          <button 
            onClick={handleLogout}
            className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600 transition"
          >
            Logout
          </button>
        </div>
        
        {/* 🔄 Render our dynamic form canvas layout for Form ID: 1 */}
        <DynamicForm token={authData.token} formId={1} />
      </div>
    </div>
  );
}

export default App;