import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DjComponent } from './dj/dj/dj.component';
import { PartyComponent } from './party/party/party.component';
import { PageNotFoundComponent } from './Shared/page-not-found/page-not-found.component';
import { StartPageComponent } from './start-page/start-page.component';

const appRoutes: Routes = [
  { path: 'dj', component: DjComponent },
  { path: 'party', component: PartyComponent, data: { title: ' - Party' } },
  { path: '', pathMatch: 'full', component: StartPageComponent },
  { path: '**', component: PageNotFoundComponent, data: { title: ' - Page Not Found' } }
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule],
  declarations: []
})
export class AppRoutingModule {}
