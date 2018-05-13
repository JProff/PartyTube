import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HistoryItem } from './../models/history-item';

const historyApiUrl = 'api/History/';
const allActionName = 'all';
const byIdActionName = 'ById/';
const byVideoIdentifierActionName = 'ByVideoIdentifier/';

@Injectable()
export class HistoryService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  all(skip: number, take: number): Observable<HistoryItem[]> {
    const url = this.baseUrl + historyApiUrl + allActionName;
    const myParams = new HttpParams().append('skip', skip.toString()).append('take', take.toString());
    return this.http.get<HistoryItem[]>(url, { params: myParams });
  }

  deleteById(id: number): Observable<any> {
    const url = this.baseUrl + historyApiUrl + byIdActionName;
    const myParams = new HttpParams().append('id', id.toString());
    return this.http.delete(url, { params: myParams });
  }

  deleteByVideoIdentifier(videoIdentifier: string): Observable<any> {
    const url = this.baseUrl + historyApiUrl + byVideoIdentifierActionName;
    const myParams = new HttpParams().append('id', videoIdentifier);
    return this.http.delete(url, { params: myParams });
  }
}
