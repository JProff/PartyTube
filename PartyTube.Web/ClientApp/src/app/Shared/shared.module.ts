import { CommonModule } from '@angular/common';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpModule } from '@ngx-progressbar/http';

import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { PlayerHubService } from './player-hub.service';

@NgModule({
  imports: [
    RouterModule,
    FormsModule,
    CommonModule,
    BrowserModule,
    HttpClientModule,
    HttpClientJsonpModule,
    NgProgressModule.forRoot(),
    NgProgressHttpModule,
    NgbModule
  ],
  declarations: [PageNotFoundComponent, ConfirmDialogComponent],
  providers: [PlayerHubService],
  entryComponents: [ConfirmDialogComponent],
  exports: [
    RouterModule,
    FormsModule,
    CommonModule,
    BrowserModule,
    HttpClientModule,
    HttpClientJsonpModule,
    NgProgressModule,
    NgbModule,
    PageNotFoundComponent,
    ConfirmDialogComponent
  ]
})
export class SharedModule {}
