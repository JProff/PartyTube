import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { SubscriptionLike as ISubscription } from 'rxjs';
import * as YouTubePlayer from 'youtube-player';

import { NowPlaying } from '../../dj/models/now-playing';
import { PlayerService } from './../../dj/services/player.service';
import { PlayerHubService } from './../../Shared/player-hub.service';

const playerStateEnded = 0;
const playerStatePlay = 1;
const playerStatePause = 2;
const timeoutAfterIncomingNowPlayingMs = 2000;

@Component({
  selector: 'app-party',
  templateUrl: './party.component.html',
  styleUrls: ['./party.component.css']
})
export class PartyComponent implements OnInit, OnDestroy, AfterViewInit {
  public playerId = 'mainPlayer';
  private player: any;
  private nowPlaying: NowPlaying;
  private hubSub: ISubscription;
  private isFromHub = false;

  constructor(private playerService: PlayerService, private playerHubService: PlayerHubService) {
    this.nowPlaying = new NowPlaying();
  }

  ngOnInit() {}

  ngAfterViewInit(): void {
    this.createPlayer();
    this.playerService.getNowPlaying().subscribe(np => this.nowPlayingChanged(np));

    this.hubSub = this.playerHubService.nowPlaying.subscribe(np => {
      this.isFromHub = true;
      this.nowPlayingChanged(np);
      setTimeout(() => {
        this.isFromHub = false;
      }, timeoutAfterIncomingNowPlayingMs);
    });
  }

  ngOnDestroy(): void {
    this.hubSub.unsubscribe();
  }

  createPlayer() {
    if (this.player) {
      return;
    }

    this.player = YouTubePlayer(this.playerId, {
      width: '100%',
      height: '100%'
    });

    this.player.on('stateChange', event => this.onPlayerStateChange(event));
    this.player.on('error', event => this.onPlayerError(event));
  }

  nowPlayingChanged(nowPlaying: NowPlaying) {
    if (nowPlaying.video) {
      const isSameVideo =
        this.nowPlaying.video && this.nowPlaying.video.videoIdentifier === nowPlaying.video.videoIdentifier;
      this.nowPlaying = nowPlaying;
      if (!isSameVideo) {
        this.player.cueVideoById({
          videoId: nowPlaying.video.videoIdentifier
        });
      }

      if (nowPlaying.isPlaying) {
        this.player.playVideo();
      } else {
        this.player.pauseVideo();
      }
    } else {
      this.nowPlaying = nowPlaying;
      this.player.stopVideo();
    }
  }

  onPlayerStateChange(event) {
    if (this.isFromHub) {
      return;
    }

    if (event.data === playerStateEnded) {
      this.playerService.playNext().subscribe();
      return;
    }

    const isStartStop = event.data === playerStatePlay || event.data === playerStatePause;
    if (!isStartStop) {
      return;
    }

    const isPlaying = event.data === playerStatePlay;
    if (this.nowPlaying.isPlaying === isPlaying) {
      return;
    }

    this.playerService.startStop(isPlaying).subscribe();
  }

  onPlayerError(event) {
    console.log(event);
  }
}
