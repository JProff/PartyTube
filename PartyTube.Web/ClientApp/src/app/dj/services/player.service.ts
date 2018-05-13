import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { NowPlaying } from './../models/now-playing';
import { VideoItem } from './../models/video-item';

const playerApiUrl = 'api/Player/';
const playNowActionName = 'PlayNow/';
const startStopActionName = 'StartStop/';
const playNextActionName = 'PlayNext/';

@Injectable()
export class PlayerService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  playNow(video: VideoItem): Observable<NowPlaying> {
    const url = this.baseUrl + playerApiUrl + playNowActionName;
    return this.http.put<NowPlaying>(url, video);
  }

  startStop(isPlaying: boolean): Observable<NowPlaying> {
    const url = this.baseUrl + playerApiUrl + startStopActionName;
    const myParams = new HttpParams().append('isPlaying', isPlaying.toString());

    return this.http.put<NowPlaying>(url, {}, { params: myParams });
  }

  playNext(): Observable<NowPlaying> {
    const url = this.baseUrl + playerApiUrl + playNextActionName;
    return this.http.put<NowPlaying>(url, {});
  }

  getNowPlaying(): Observable<NowPlaying> {
    const url = this.baseUrl + playerApiUrl;
    return this.http.get<NowPlaying>(url);
  }
}
