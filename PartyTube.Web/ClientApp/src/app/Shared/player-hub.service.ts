import { EventEmitter, Inject, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { ReplaySubject } from 'rxjs';

import { NowPlaying } from '../dj/models/now-playing';

const hubName = 'Hubs/PlayerHub';
const nowPlayingMethodName = 'NowPlaying';
const currentPlaylistMethodName = 'CurrentPlaylist';
const clearCurrentPlaylistMethodName = 'ClearCurrentPlaylist';

@Injectable()
export class PlayerHubService {
  private hubConnection: HubConnection;
  private baseUrl: string;

  private nowPlayingSource = new ReplaySubject<NowPlaying>(1);
  public nowPlaying = this.nowPlayingSource.asObservable();

  public currentPlaylist = new EventEmitter();
  public clearCurrentPlaylist = new EventEmitter();

  constructor(@Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

    const url = this.baseUrl + hubName;
    this.hubConnection = new HubConnectionBuilder().withUrl(url).build();

    this.hubConnection.on(nowPlayingMethodName, (np: NowPlaying) => {
      this.nowPlayingSource.next(np);
    });

    this.hubConnection.on(currentPlaylistMethodName, () => {
      this.currentPlaylist.next({});
    });

    this.hubConnection.on(clearCurrentPlaylistMethodName, () => {
      this.clearCurrentPlaylist.next({});
    });

    this.hubConnection.start();
  }
}
