import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DjModule } from './dj/dj.module';
import { MyFooterComponent } from './my-footer/my-footer.component';
import { PartyModule } from './party/party.module';
import { SharedModule } from './Shared/shared.module';
import { StartPageComponent } from './start-page/start-page.component';

@NgModule({
  declarations: [AppComponent, StartPageComponent, MyFooterComponent],
  imports: [NgbModule.forRoot(), SharedModule, DjModule, PartyModule, AppRoutingModule],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
