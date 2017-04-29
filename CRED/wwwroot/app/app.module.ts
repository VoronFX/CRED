import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { NgModule, enableProdMode } from "@angular/core";
import { BrowserModule, Title } from "@angular/platform-browser";
import { routing, routedComponents } from "./app.routing";
import { APP_BASE_HREF, Location } from "@angular/common";
import { AppComponent } from "./app.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpModule  } from "@angular/http";
import { SampleDataService } from "./services/sampleData.service";
import { AuthService } from "./security/auth.service";
import { AuthGuard } from "./security/auth-guard.service";
import { ToastrModule } from "ngx-toastr";
import "./rxjs-operators";
import { BladeUiModule, BladeUiService, PortalComponent } from "./bladeui/definitions";

// enableProdMode();

@NgModule({
    imports: [BrowserAnimationsModule, BrowserModule, FormsModule, HttpModule, ToastrModule.forRoot(), routing, BladeUiModule],
    declarations: [AppComponent, routedComponents, PortalComponent],
    providers: [SampleDataService,
        AuthService, BladeUiService,
        AuthGuard, Title, { provide: APP_BASE_HREF, useValue: "/a2spa" }],
    bootstrap: [AppComponent]
})
export class AppModule { }
