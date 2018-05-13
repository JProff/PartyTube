import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { NgbTypeaheadModule } from '@ng-bootstrap/ng-bootstrap';
import { DragulaModule } from 'ng2-dragula/ng2-dragula';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { SharedModule } from './../Shared/shared.module';
import { DjRoutingModule } from './dj-routing.module';
import { DjComponent } from './dj/dj.component';
import { HistoryComponent } from './history/history.component';
import { PlaylistComponent } from './playlist/playlist.component';
import { SearchComponent } from './search/search.component';
import { CurrentPlaylistService } from './services/current-playlist.service';
import { HistoryService } from './services/history.service';
import { PlayerService } from './services/player.service';
import { SearchService } from './services/search.service';
import { VideoItemComponent } from './video-item/video-item.component';

@NgModule({
  imports: [
    SharedModule,
    ReactiveFormsModule,
    NgbTypeaheadModule,
    DjRoutingModule,
    InfiniteScrollModule,
    DragulaModule
  ],
  declarations: [
    DjComponent,
    PlaylistComponent,
    VideoItemComponent,
    HistoryComponent,
    SearchComponent
  ],
  providers: [
    SearchService,
    HistoryService,
    CurrentPlaylistService,
    PlayerService
  ]
})
export class DjModule {}
