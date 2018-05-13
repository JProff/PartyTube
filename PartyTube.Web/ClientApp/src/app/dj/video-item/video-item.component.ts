import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { v4 as uuid } from 'uuid';
import * as YouTubePlayer from 'youtube-player';

import { environment } from './../../../environments/environment';
import { VideoItem } from './../models/video-item';

@Component({
  selector: 'app-video-item',
  templateUrl: './video-item.component.html',
  styleUrls: ['./video-item.component.css']
})
export class VideoItemComponent implements OnInit {
  @Input() videoItem: VideoItem;
  public isPlayerVisible: boolean;
  public playerId: string;
  private player: any;
  private startPercent = environment.videoItemPreviewStartPercent;
  @ViewChild('videoItemContainer') videoItemContainer: ElementRef;

  constructor() {
    this.isPlayerVisible = false;
    this.playerId = 'video-player-' + uuid();
  }

  ngOnInit() {}

  play() {
    this.isPlayerVisible = !this.isPlayerVisible;
    if (!this.isPlayerVisible) {
      this.player.destroy();
      return;
    }

    this.player = YouTubePlayer(this.playerId, {
      width: this.videoItemContainer.nativeElement.clientWidth,
      height: this.videoItemContainer.nativeElement.clientWidth * 9 / 16
    });
    this.player.loadVideoById({
      videoId: this.videoItem.videoIdentifier,
      startSeconds: this.videoItem.durationInSeconds / this.startPercent
    });
    this.player.playVideo();
  }
}
