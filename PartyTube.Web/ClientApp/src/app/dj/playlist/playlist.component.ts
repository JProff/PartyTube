import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DragulaService } from 'ng2-dragula/ng2-dragula';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { ConfirmDialogComponent } from './../../Shared/confirm-dialog/confirm-dialog.component';
import { PlayerHubService } from './../../Shared/player-hub.service';
import { CurrentPlaylistVideos } from './../models/current-playlist-videos';
import { CurrentPlaylistService } from './../services/current-playlist.service';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css']
})
export class PlaylistComponent implements OnInit, OnDestroy {
  public currentPlaylistVideos: CurrentPlaylistVideos;
  public isLoading: boolean;
  private hubSubCurrentPlaylist: ISubscription;
  private hubSubClearCurrentPlaylist: ISubscription;

  constructor(
    private currentPlaylistService: CurrentPlaylistService,
    private modalService: NgbModal,
    private dragulaService: DragulaService,
    private playerHubService: PlayerHubService
  ) {
    this.isLoading = true;
    this.clearCurrentPlaylistVideos();
  }

  private onDrop() {
    const ids = [];
    this.currentPlaylistVideos.currentPlaylistItems.forEach(item => ids.push(item.id));

    this.currentPlaylistService.reorder(ids).subscribe();
  }

  updateCurrentPlaylistVideos() {
    this.currentPlaylistService
      .getAll()
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe(videos => {
        this.currentPlaylistVideos = videos;
      });
  }

  clearCurrentPlaylistVideos() {
    this.currentPlaylistVideos = {
      totalMinutes: 0,
      totalSeconds: 0,
      currentPlaylistItems: new Array(0)
    };
  }

  ngOnInit() {
    this.dragulaService.setOptions('bag-one', {
      moves: function(el, container, handle) {
        return handle.classList.contains('videoItemThumbnail');
      }
    });

    this.dragulaService.dropModel.subscribe(value => {
      this.onDrop();
    });

    this.updateCurrentPlaylistVideos();
    this.hubSubCurrentPlaylist = this.playerHubService.currentPlaylist.subscribe(() => {
      this.updateCurrentPlaylistVideos();
    });

    this.hubSubClearCurrentPlaylist = this.playerHubService.clearCurrentPlaylist.subscribe(() => {
      this.clearCurrentPlaylistVideos();
    });
  }

  clear() {
    const modalRef = this.modalService.open(ConfirmDialogComponent);
    modalRef.componentInstance.message = 'Delete all items from current playlist?';
    modalRef.result.then(
      result => {
        if (result) {
          this.currentPlaylistService.clear().subscribe();
        }
      },
      () => {}
    );
  }

  ngOnDestroy(): void {
    this.dragulaService.destroy('bag-one');
    this.hubSubCurrentPlaylist.unsubscribe();
    this.hubSubClearCurrentPlaylist.unsubscribe();
  }

  trackByFn(index, item) {
    return item.id;
  }

  remove(id: number, i: number) {
    const modalRef = this.modalService.open(ConfirmDialogComponent);
    modalRef.componentInstance.message = `Remove <b>${
      this.currentPlaylistVideos.currentPlaylistItems[i].video.title
    }</b> from current playlist?`;
    modalRef.result.then(
      result => {
        if (result) {
          this.currentPlaylistService.remove(id).subscribe();
        }
      },
      () => {}
    );
  }
}
