import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbTypeaheadConfig } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of, SubscriptionLike as ISubscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import * as youtubeRegex from 'youtube-regex';

import { environment } from '../../../environments/environment';
import { NowPlaying } from '../models/now-playing';
import { SearchPopupResult } from '../models/search-popup-result';
import { CurrentPlaylistService } from '../services/current-playlist.service';
import { PlayerService } from '../services/player.service';
import { SearchService } from '../services/search.service';
import { PlayerHubService } from './../../Shared/player-hub.service';

@Component({
  selector: 'app-dj',
  templateUrl: './dj.component.html',
  styleUrls: ['./dj.component.css'],
  providers: [NgbTypeaheadConfig]
})
export class DjComponent implements OnInit, OnDestroy {
  public isVideoLink = false;
  public model: any;
  public nowPlaying: NowPlaying;
  private hubSub: ISubscription;
  @ViewChild('searchInput') searchInput: ElementRef;

  public search = (text$: Observable<string>) =>
    text$.pipe(
      tap((term: string) => {
        this.isVideoLink = this.checkIsVideoLink(term);
        return !this.isVideoLink;
      }),
      debounceTime(environment.debounceTimeMs),
      distinctUntilChanged(),
      switchMap((term: string) => (this.isVideoLink ? of([]) : this.searchService.searchPopup(term)))
      // tslint:disable-next-line:semicolon
    );

  searchPopupFormatter(res: SearchPopupResult) {
    return res.videoName;
  }
  constructor(
    private searchService: SearchService,
    config: NgbTypeaheadConfig,
    private router: Router,
    private route: ActivatedRoute,
    private playerService: PlayerService,
    private playerHubService: PlayerHubService,
    private currentPlaylistService: CurrentPlaylistService
  ) {
    config.showHint = true;
    config.focusFirst = false;
  }

  onSearchClick(event) {
    if (event) {
      event.preventDefault();
    }

    this.model = this.searchInput.nativeElement.value;

    this.onSubmit();
  }

  clearSearch() {
    this.isVideoLink = false;
    this.model = null;
  }
  checkIsVideoLink(text: string) {
    return youtubeRegex().test(text);
  }

  ngOnInit() {
    this.playerService.getNowPlaying().subscribe(np => (this.nowPlaying = np));
    this.hubSub = this.playerHubService.nowPlaying.subscribe(np => (this.nowPlaying = np));
  }

  ngOnDestroy(): void {
    this.hubSub.unsubscribe();
  }

  addVideoLinkToPlaylist() {
    this.currentPlaylistService.addToEndIdOrUrl(this.model).subscribe();
    this.clearSearch();
  }

  showSearchResults() {
    let term = this.model;
    if (this.model.videoName) {
      term = this.model.videoName;
    }

    this.router.navigate(['search/', term], { relativeTo: this.route });
  }

  startStop() {
    this.playerService.startStop(!this.nowPlaying.isPlaying).subscribe();
  }

  playNext() {
    this.playerService.playNext().subscribe();
  }

  onSubmit() {
    if (this.isVideoLink) {
      this.addVideoLinkToPlaylist();
    } else {
      this.showSearchResults();
    }
  }
}
