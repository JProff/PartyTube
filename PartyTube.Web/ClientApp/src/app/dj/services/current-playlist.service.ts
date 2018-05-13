import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { CurrentPlaylistVideos } from './../models/current-playlist-videos';
import { VideoItem } from './../models/video-item';

const currentPlaylistApiUrl = 'api/CurrentPlaylist/';
const addToEndActionName = 'AddToEnd/';
const addToStartActionName = 'AddToStart/';
const getAllActionName = 'GetAll/';
const reorderActionName = 'Reorder/';
const removeActionName = 'Remove/';
const addToEndIdOrUrlActionName = 'AddToEndIdOrUrl/';
const clearActionName = 'Clear/';

@Injectable()
export class CurrentPlaylistService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  addToEndIdOrUrl(idOrUrl: string): Observable<any> {
    const url = this.baseUrl + currentPlaylistApiUrl + addToEndIdOrUrlActionName;
    const myParams = new HttpParams().append('idOrUrl', idOrUrl);
    return this.http.put(url, {}, { params: myParams });
  }
  addToEnd(video: VideoItem): Observable<any> {
    const url = this.baseUrl + currentPlaylistApiUrl + addToEndActionName;
    return this.http.put(url, video);
  }

  addToStart(video: VideoItem): Observable<any> {
    const url = this.baseUrl + currentPlaylistApiUrl + addToStartActionName;
    return this.http.put(url, video);
  }

  getAll(): Observable<CurrentPlaylistVideos> {
    const url = this.baseUrl + currentPlaylistApiUrl + getAllActionName;
    return this.http.get<CurrentPlaylistVideos>(url);
  }

  remove(id: number): Observable<any> {
    const url = this.baseUrl + currentPlaylistApiUrl + removeActionName;
    const myParams = new HttpParams().append('id', id.toString());
    return this.http.delete(url, { params: myParams });
  }

  reorder(ids: number[]): Observable<any> {
    const url = this.baseUrl + currentPlaylistApiUrl + reorderActionName;
    return this.http.put(url, ids);
  }

  clear(): Observable<any> {
    const url = this.baseUrl + currentPlaylistApiUrl + clearActionName;
    return this.http.delete(url);
  }
}
