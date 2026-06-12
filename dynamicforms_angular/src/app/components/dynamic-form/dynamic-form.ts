import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FormService } from '../../services/form';

@Component({
  selector: 'app-dynamic-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dynamic-form.html',
  styleUrls: ['./dynamic-form.css']
})
export class DynamicFormComponent implements OnInit {
  formFields: any[] = [];
  formName: string = '';
  formData: any = {}; // Stores user responses key-value style

  constructor(private formService: FormService) {}

  ngOnInit(): void {
    const targetFormId = 1; // Example Form ID
    
    this.formService.getFormMetadata(targetFormId).subscribe({
      next: (response) => {
        this.formName = response.formName;
        // The backend filters out 'Hidden' fields entirely, so we just render what arrives
        this.formFields = response.fields; 
      },
      error: (err) => console.error('Error fetching form data', err)
    });
  }

  onSubmit(): void {
    const targetFormId = 1;
    this.formService.submitFormResponse(targetFormId, this.formData).subscribe({
      next: (res) => alert('Form submitted successfully!'),
      error: (err) => alert('Submission failed or unauthorized')
    });
  }
}