import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbPanelChangeEvent } from '@ng-bootstrap/ng-bootstrap';

import { environment } from './../../../environments/environment';
import { LocalSearchVideoItem } from './../models/local-search-video-item';
import { VideoItem } from './../models/video-item';
import { CurrentPlaylistService } from './../services/current-playlist.service';
import { PlayerService } from './../services/player.service';
import { SearchService } from './../services/search.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  public term: string;
  public localVideos: LocalSearchVideoItem[];
  public youtubeVideos: VideoItem[];
  public youtubeTotal: number;
  private youtubeNextPageToken: string;
  public pageLength = environment.youtubeSearchPageLength;
  public throttle = environment.infiniteScrollThrottle;
  public scrollDistance = environment.infiniteScrollDistance;
  public isFirstLoad: boolean;

  constructor(
    private route: ActivatedRoute,
    private searchService: SearchService,
    private currentPlaylistService: CurrentPlaylistService,
    private playerService: PlayerService
  ) {
    this.localVideos = new Array(0);
    this.youtubeVideos = new Array(0);
    this.isFirstLoad = true;
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.isFirstLoad = true;
      this.term = params['term'];
      this.searchService
        .getLocal(this.term)
        .subscribe(local => (this.localVideos = local));
      this.youtubeVideos = new Array(0);
      this.youtubeNextPageToken = '';
      this.onScrollDown();
    });
  }

  onScrollDown() {
    this.searchService
      .getExternal(this.term, this.pageLength, this.youtubeNextPageToken)
      .subscribe(res => {
        this.youtubeNextPageToken = res.nextPageToken;
        this.youtubeTotal = res.total;
        res.videos.forEach(v => this.youtubeVideos.push(v));
        this.isFirstLoad = false;
      });
  }

  localTrackByFn(index, item) {
    return item.videoItem.id;
  }

  youtubeTrackByFn(index, item) {
    return index;
  }

  addToCurrentPlaylist(video: VideoItem) {
    this.currentPlaylistService.addToEnd(video).subscribe();
  }

  playNext(video: VideoItem) {
    this.currentPlaylistService.addToStart(video).subscribe();
  }
  playNow(video: VideoItem) {
    this.playerService.playNow(video).subscribe();
  }

  public beforeChange($event: NgbPanelChangeEvent) {
    if (
      $event.panelId === 'youtubeResultsPanel' &&
      $event.nextState === false
    ) {
      $event.preventDefault();
    }
  }
}
