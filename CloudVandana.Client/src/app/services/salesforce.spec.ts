import { TestBed } from '@angular/core/testing';
import { Salesforce } from './salesforce';

describe('Salesforce', () => {
  let service: Salesforce;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Salesforce);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
