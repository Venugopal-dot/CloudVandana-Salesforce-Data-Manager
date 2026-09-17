import {
  ChangeDetectorRef,
  Component,
  NgZone,
  OnInit
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import { SalesforceService } from './services/salesforce';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  // =====================================================
  // SALESFORCE OBJECTS
  // =====================================================

  objects = [
    'Account',
    'Opportunity',
    'Lead',
    'Contact',
    'Case'
  ];


  // =====================================================
  // DYNAMIC FIELD CONFIGURATION
  // =====================================================

  fieldConfig: {
    [objectName: string]: {
      [fieldName: string]: {
        type: string;
        required?: boolean;
        options?: string[];
      };
    };
  } = {

      Account: {

        Name: {
          type: 'text',
          required: true
        },

        Phone: {
          type: 'text'
        },

        Website: {
          type: 'url'
        },

        Industry: {
          type: 'text'
        }

      },


      Opportunity: {

        Name: {
          type: 'text',
          required: true
        },

        Amount: {
          type: 'number'
        },

        StageName: {
          type: 'select',
          required: true,
          options: [
            'Prospecting',
            'Qualification',
            'Needs Analysis',
            'Value Proposition',
            'Id. Decision Makers',
            'Perception Analysis',
            'Proposal/Price Quote',
            'Negotiation/Review',
            'Closed Won',
            'Closed Lost'
          ]
        },

        CloseDate: {
          type: 'date',
          required: true
        }

      },


      Lead: {

        FirstName: {
          type: 'text'
        },

        LastName: {
          type: 'text',
          required: true
        },

        Company: {
          type: 'text',
          required: true
        },

        Email: {
          type: 'email'
        },

        Status: {
          type: 'select',
          options: [
            'Open - Not Contacted',
            'Working - Contacted',
            'Closed - Converted',
            'Closed - Not Converted'
          ]
        }

      },


      Contact: {

        FirstName: {
          type: 'text'
        },

        LastName: {
          type: 'text'
        },

        Email: {
          type: 'email'
        },

        Phone: {
          type: 'text'
        },

        Title: {
          type: 'text'
        }

      },


      Case: {

        Subject: {
          type: 'text',
          required: true
        },

        Status: {
          type: 'select',
          required: true,
          options: [
            'New',
            'Working',
            'Escalated',
            'Closed'
          ]
        },

        Priority: {
          type: 'select',
          required: true,
          options: [
            'High',
            'Medium',
            'Low'
          ]
        }

      }

    };

  // =====================================================
  // APPLICATION STATE
  // =====================================================

  selectedObject = 'Account';

  isLoggedIn = false;

  fields: string[] = [];

  records: any[] = [];

  page = 1;

  pageSize = 20;

  hasNextPage = false;

  loading = false;

  errorMessage = '';

  showCreateForm = false;

  editingRecordId: string | null = null;

  createFields: {
    [key: string]: any;
  } = {};

  selectedRecord: any = null;


  // Prevent duplicate Create / Update clicks
  isSaving = false;


  // =====================================================
  // CONSTRUCTOR
  // =====================================================

  constructor(
    private salesforceService: SalesforceService,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) { }


  // =====================================================
  // INITIALIZATION
  // =====================================================

  ngOnInit(): void {

    if (typeof window === 'undefined') {
      return;
    }

    const params = new URLSearchParams(
      window.location.search
    );

    // ---------------------------------------------
    // After Salesforce OAuth login
    // ---------------------------------------------

    if (params.get('login') === 'success') {

      this.isLoggedIn = true;

      this.loadData();

      window.history.replaceState(
        {},
        document.title,
        '/'
      );

      return;
    }


    // ---------------------------------------------
    // Check existing ASP.NET session
    // ---------------------------------------------

    this.salesforceService
      .getAuthStatus()
      .subscribe({

        next: (response) => {

          console.log(
            'Authentication status:',
            response
          );


          if (response.isLoggedIn) {

            this.isLoggedIn = true;

            this.loadData();

          }

        },


        error: (error) => {

          console.error(
            'Authentication status error:',
            error
          );

          this.isLoggedIn = false;

        }

      });

  }


  // =====================================================
  // SALESFORCE LOGIN
  // =====================================================

  login(): void {
    window.location.href = 'https://cloudvandana-salesforce-data-manager.onrender.com/api/auth/login';
  }


  // =====================================================
  // LOAD DATA
  // =====================================================

  loadData(): void {

    this.loading = true;
    this.errorMessage = '';

    this.salesforceService
      .getFields(this.selectedObject)
      .subscribe({

        next: (fields) => {

          this.fields = [...fields];

          this.salesforceService
            .getRecords(
              this.selectedObject,
              this.page,
              this.pageSize
            )
            .subscribe({

              next: (response) => {

                console.log(
                  'Fresh Salesforce response:',
                  response
                );

                const newRecords =
                  response?.records ?? [];

                console.log(
                  'Fresh Salesforce records:',
                  newRecords
                );

                this.ngZone.run(() => {

                  this.records = [...newRecords];

                  this.hasNextPage =
                    newRecords.length === this.pageSize;

                  this.loading = false;

                  this.cdr.markForCheck();
                  this.cdr.detectChanges();

                });

              },

              error: (error) => {

                console.error(
                  'Records loading error:',
                  error
                );

                this.ngZone.run(() => {

                  this.records = [];
                  this.hasNextPage = false;

                  this.errorMessage =
                    'Unable to load Salesforce records.';

                  this.loading = false;

                  this.cdr.markForCheck();
                  this.cdr.detectChanges();

                });

              }

            });

        },

        error: (error) => {

          console.error(
            'Fields loading error:',
            error
          );

          this.ngZone.run(() => {

            this.fields = [];
            this.records = [];
            this.hasNextPage = false;

            this.errorMessage =
              'Unable to load Salesforce fields.';

            this.loading = false;

            this.cdr.markForCheck();
            this.cdr.detectChanges();

          });

        }

      });

  }

  // =====================================================
  // OBJECT CHANGE
  // =====================================================

  objectChanged(): void {

    this.page = 1;

    this.selectedRecord = null;

    this.showCreateForm = false;

    this.editingRecordId = null;

    this.createFields = {};

    this.errorMessage = '';

    this.loadData();

  }


  // =====================================================
  // NEXT PAGE
  // =====================================================

  nextPage(): void {

    if (!this.hasNextPage) {
      return;
    }

    this.page++;

    this.loadData();

  }


  // =====================================================
  // PREVIOUS PAGE
  // =====================================================

  previousPage(): void {

    if (this.page > 1) {

      this.page--;

      this.loadData();

    }

  }


  // =====================================================
  // VIEW RECORD
  // =====================================================

  viewRecord(record: any): void {

    this.selectedRecord = record;

  }


  // =====================================================
  // EDIT RECORD
  // =====================================================

  editRecord(record: any): void {

    this.editingRecordId = record.Id;

    this.selectedRecord = record;


    this.createFields = {
      ...record
    };


    this.showCreateForm = true;

    this.errorMessage = '';

  }


  // =====================================================
  // DELETE RECORD
  // =====================================================

  deleteRecord(record: any): void {

    const confirmed = confirm(
      `Are you sure you want to delete this ${this.selectedObject}?`
    );


    if (!confirmed) {
      return;
    }


    this.salesforceService
      .deleteRecord(
        this.selectedObject,
        record.Id
      )
      .subscribe({

        next: () => {

          console.log(
            'Deleted Salesforce Record:',
            record.Id
          );


          // Remove deleted record
          // from Angular table
          this.records = this.records.filter(
            item => item.Id !== record.Id
          );


          // Clear selected record
          if (
            this.selectedRecord &&
            this.selectedRecord.Id === record.Id
          ) {

            this.selectedRecord = null;

          }


          // Force Angular UI refresh
          this.cdr.detectChanges();


          alert(
            `${this.selectedObject} deleted successfully.`
          );

        },


        error: (error) => {

          console.error(
            'Delete error:',
            error
          );


          alert(
            `Unable to delete ${this.selectedObject}.`
          );

        }

      });

  }


  // =====================================================
  // OPEN CREATE FORM
  // =====================================================

  openCreateForm(): void {

    this.editingRecordId = null;

    this.selectedRecord = null;

    this.createFields = {};

    this.errorMessage = '';

    this.showCreateForm = true;

  }


  // =====================================================
  // CREATE RECORD
  // =====================================================

  createRecord(): void {

    if (this.isSaving) {
      return;
    }

    // Validate required fields
    const config =
      this.fieldConfig[this.selectedObject];

    for (const field of this.fields) {

      if (field === 'Id') {
        continue;
      }

      const fieldSettings = config?.[field];

      if (
        fieldSettings?.required &&
        (
          this.createFields[field] === undefined ||
          this.createFields[field] === null ||
          String(this.createFields[field]).trim() === ''
        )
      ) {

        this.errorMessage =
          `${field} is required.`;

        return;
      }
    }


    this.isSaving = true;

    this.errorMessage = '';


    this.salesforceService
      .createRecord(
        this.selectedObject,
        this.createFields
      )
      .subscribe({

        next: (response) => {

          console.log(
            'Create successful:',
            response
          );

          this.isSaving = false;

          // Close the modal
          this.showCreateForm = false;
          this.selectedRecord = null;
          this.editingRecordId = null;
          this.createFields = {};

          // Return to first page after creating
          this.page = 1;

          // Refresh records
          this.loadData();

          // Refresh UI
          this.cdr.detectChanges();

        },


        error: (error) => {

          console.error(
            'Create error:',
            error
          );

          this.isSaving = false;

          this.errorMessage =
            `Unable to create ${this.selectedObject}.`;

          this.cdr.detectChanges();

        }

      });

  }


  // =====================================================
  // UPDATE RECORD
  // =====================================================

  updateRecord(): void {

    if (this.isSaving) {
      return;
    }


    const recordId =
      this.editingRecordId ??
      this.selectedRecord?.Id;


    if (!recordId) {

      alert(
        'Record ID is missing.'
      );

      return;
    }


    // Validate required fields
    const config =
      this.fieldConfig[this.selectedObject];

    for (const field of this.fields) {

      if (field === 'Id') {
        continue;
      }

      const fieldSettings = config?.[field];

      if (
        fieldSettings?.required &&
        (
          this.createFields[field] === undefined ||
          this.createFields[field] === null ||
          String(this.createFields[field]).trim() === ''
        )
      ) {

        this.errorMessage =
          `${field} is required.`;

        return;
      }
    }


    // Remove Id before sending to Salesforce
    const {
      Id,
      ...updateFields
    } = this.createFields;


    this.isSaving = true;

    this.errorMessage = '';


    this.salesforceService
      .updateRecord(
        this.selectedObject,
        recordId,
        updateFields
      )
      .subscribe({

        next: () => {

          console.log(
            'Record updated successfully:',
            recordId
          );


          this.records = this.records.map(
            record =>
              record.Id === recordId
                ? {
                  ...record,
                  ...updateFields
                }
                : record
          );


          this.showCreateForm = false;

          this.editingRecordId = null;

          this.selectedRecord = null;

          this.createFields = {};

          this.isSaving = false;

          this.cdr.detectChanges();

        },


        error: (error) => {

          this.isSaving = false;

          console.error(
            'Update error:',
            error
          );

          this.errorMessage =
            `Unable to update ${this.selectedObject}.`;

          alert(
            `Update failed: ${error.message}`
          );

        }

      });

  }

}