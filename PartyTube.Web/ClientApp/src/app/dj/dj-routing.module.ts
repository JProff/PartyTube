import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DjComponent } from './dj/dj.component';
import { HistoryComponent } from './history/history.component';
import { PlaylistComponent } from './playlist/playlist.component';
import { SearchComponent } from './search/search.component';

const djRoutes: Routes = [
  {
    path: 'dj',
    component: DjComponent,
    children: [
      { path: 'history', component: HistoryComponent, data: { title: ' - History' } },
      { path: 'search/:term', component: SearchComponent, data: { title: ' - Search' } },
      { path: 'search', component: SearchComponent, data: { title: ' - Search' } },
      { path: '', component: PlaylistComponent, data: { title: ' - Current Playlist' } }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(djRoutes)],
  exports: [RouterModule]
})
export class DjRoutingModule {}
