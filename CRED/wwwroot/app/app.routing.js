"use strict";
var router_1 = require("@angular/router");
var auth_guard_service_1 = require("./security/auth-guard.service");
var about_component_1 = require("./about.component");
var index_component_1 = require("./index.component");
var contact_component_1 = require("./contact.component");
var login_component_1 = require("./login.component");
var register_component_1 = require("./register.component");
var fileupload_component_1 = require("./fileupload.component");
var test_component_1 = require("./components/test.component");
var appRoutes = [
    { path: "", redirectTo: "test", pathMatch: "full" },
    { path: "home", component: index_component_1.IndexComponent, data: { title: "Home" } },
    { path: "login", component: login_component_1.LoginComponent, data: { title: "Login" } },
    { path: "register", component: register_component_1.RegisterComponent, data: { title: "Register" } },
    { path: "about", component: about_component_1.AboutComponent, data: { title: "About" }, canActivate: [auth_guard_service_1.AuthGuard] },
    { path: "contact", component: contact_component_1.ContactComponent, data: { title: "Contact" } },
    { path: "fileupload", component: fileupload_component_1.FileUploadComponent, data: { title: "FileUpload" } },
    { path: "test", component: test_component_1.TestComponent, data: { title: "Test" } }
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
exports.routedComponents = [about_component_1.AboutComponent, index_component_1.IndexComponent, contact_component_1.ContactComponent, login_component_1.LoginComponent, register_component_1.RegisterComponent, fileupload_component_1.FileUploadComponent, test_component_1.TestComponent];
//# sourceMappingURL=app.routing.js.map