import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { LocalSearchVideoItem } from '../models/local-search-video-item';
import { SearchPopupResult } from '../models/search-popup-result';
import { YoutubeSearchResult } from '../models/youtube-search-result';

const searchApiUrl = 'api/Search/';
const popupActionName = 'popup/';
const localActionName = 'local/';
const externalActionName = 'external/';

@Injectable()
export class SearchService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  searchPopup(term: string): Observable<SearchPopupResult[]> {
    if (!term || !term.trim()) {
      return of([]);
    }

    const url = this.baseUrl + searchApiUrl + popupActionName;
    const myParams = new HttpParams().append('term', term);
    return this.http.get<SearchPopupResult[]>(url, { params: myParams });
  }

  getLocal(term: string): Observable<LocalSearchVideoItem[]> {
    if (!term || !term.trim()) {
      return of([]);
    }

    const url = this.baseUrl + searchApiUrl + localActionName;
    const myParams = new HttpParams().append('term', term);
    return this.http.get<LocalSearchVideoItem[]>(url, { params: myParams });
  }

  getExternal(term: string, count: number, pageToken: string): Observable<YoutubeSearchResult> {
    if (!term || !term.trim()) {
      return of();
    }

    const url = this.baseUrl + searchApiUrl + externalActionName;
    let myParams = new HttpParams().append('term', term).append('count', count.toString());
    if (pageToken != null) {
      myParams = myParams.append('pageToken', pageToken);
    }
    return this.http.get<YoutubeSearchResult>(url, { params: myParams });
  }
}
