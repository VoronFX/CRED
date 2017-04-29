import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "./security/auth-guard.service";

import { AboutComponent } from "./about.component";
import { IndexComponent } from "./index.component";
import { ContactComponent } from "./contact.component";
import { LoginComponent } from "./login.component";
import { RegisterComponent } from "./register.component";
import { FileUploadComponent } from "./fileupload.component";
import { TestComponent } from "./components/test.component";

const appRoutes: Routes = [
    { path: "", redirectTo: "test", pathMatch: "full" },
    { path: "home", component: IndexComponent, data: { title: "Home" } },
    { path: "login", component: LoginComponent, data: { title: "Login" } },
    { path: "register", component: RegisterComponent, data: { title: "Register" } },
    { path: "about", component: AboutComponent, data: { title: "About" }, canActivate: [AuthGuard]  },
    { path: "contact", component: ContactComponent, data: { title: "Contact" }},
    { path: "fileupload", component: FileUploadComponent, data: { title: "FileUpload" }},
    { path: "test", component: TestComponent, data: { title: "Test" }}
];

export const routing = RouterModule.forRoot(appRoutes);

export const routedComponents = [AboutComponent, IndexComponent, ContactComponent, LoginComponent, RegisterComponent, FileUploadComponent, TestComponent ];
