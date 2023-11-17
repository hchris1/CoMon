import { TestBed } from '@angular/core/testing';

import { CoMonHubService } from './co-mon-hub.service';

describe('CoMonHubService', () => {
  let service: CoMonHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CoMonHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
