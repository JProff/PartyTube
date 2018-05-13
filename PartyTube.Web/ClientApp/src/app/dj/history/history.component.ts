import { ConfirmDialogComponent } from './../../Shared/confirm-dialog/confirm-dialog.component';
import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { PlayerService } from '../services/player.service';
import { environment } from './../../../environments/environment';
import { HistoryItem } from './../models/history-item';
import { VideoItem } from './../models/video-item';
import { CurrentPlaylistService } from './../services/current-playlist.service';
import { HistoryService } from './../services/history.service';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit {
  public historyItems: HistoryItem[];
  pageLength = environment.historyPageLength;
  throttle = environment.infiniteScrollThrottle;
  scrollDistance = environment.infiniteScrollDistance;

  constructor(
    private historyService: HistoryService,
    private currentPlaylistService: CurrentPlaylistService,
    private playerService: PlayerService,
    private modalService: NgbModal
  ) {
    this.historyItems = new Array(0);
  }

  ngOnInit() {
    this.onScrollDown();
  }

  trackByFn(index, item) {
    return item.id;
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
  removeFromHistory(i: number, id: number) {
    const modalRef = this.modalService.open(ConfirmDialogComponent);
    modalRef.componentInstance.message = `Remove <b>${
      this.historyItems[i].video.title
    }</b> from history?`;
    modalRef.result.then(
      result => {
        if (result) {
          this.historyService
            .deleteById(id)
            .subscribe(_ => this.historyItems.splice(i, 1));
        }
      },
      () => {}
    );
  }

  removeAllEntriesFromHistory(videoIdentifier: string, i: number) {
    const modalRef = this.modalService.open(ConfirmDialogComponent);
    modalRef.componentInstance.message = `Remove all entries <b>${
      this.historyItems[i].video.title
    }</b> from history?`;
    modalRef.result.then(
      result => {
        if (result) {
          this.historyService
            .deleteByVideoIdentifier(videoIdentifier)
            .subscribe(
              _ =>
                (this.historyItems = this.historyItems.filter(
                  f => f.video.videoIdentifier !== videoIdentifier
                ))
            );
        }
      },
      () => {}
    );
  }

  onScrollDown() {
    const skip = this.historyItems.length;
    const take = this.pageLength;
    this.historyService
      .all(skip, take)
      .subscribe(items => items.forEach(i => this.historyItems.push(i)));
  }
}
