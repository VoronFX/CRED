import { Component, AfterViewInit } from "@angular/core";
import { BladeUiModule, BladeUiService, PortalComponent, Favorite, Blade } from "../bladeui/definitions";

@Component({
    selector: "test-component",
    templateUrl: "component/TestComponent"
})
export class TestComponent implements AfterViewInit {

    private bladeUiService: BladeUiService;

    constructor(bladeUiService: BladeUiService) {
        this.bladeUiService = bladeUiService;
    }

    ngAfterViewInit() {
        //this.bladeUiService.sidebar.favorites = [
        //    new Favorite({
        //        label: "New Blade", uri: "javaScript:void(0);", opensExternal: false, isFavorite: false, action: ()=> {
        //        this.bladeUiService.panorama.openBlade(new Blade(), this);
        //    }}),
        //    new Favorite({ label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        //    new Favorite({ label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        //    new Favorite({ label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        //];
    }
}