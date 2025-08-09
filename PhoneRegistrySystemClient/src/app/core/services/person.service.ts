import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Person, PersonSummary, CreatePersonRequest, AddContactInfoRequest } from '../models/person.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private readonly baseUrl = `${environment.contactApiUrl}/api/persons`;

  constructor(private http: HttpClient) {}

  getPersons(skip: number = 0, take: number = 50): Observable<PersonSummary[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    
    return this.http.get<PersonSummary[]>(this.baseUrl, { params });
  }

  getPerson(id: string): Observable<Person> {
    return this.http.get<Person>(`${this.baseUrl}/${id}`);
  }

  createPerson(request: CreatePersonRequest): Observable<Person> {
    return this.http.post<Person>(this.baseUrl, request);
  }

  deletePerson(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  addContactInfo(personId: string, request: AddContactInfoRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/${personId}/contact-infos`, request);
  }

  removeContactInfo(personId: string, contactInfoId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${personId}/contact-infos/${contactInfoId}`);
  }
}
