import { Component, ViewEncapsulation } from "@angular/core";

import { Resources, MsPortalFxImages, BladeUiService, Blade, BladeComponent } from "../../definitions";
//import { TestBladeComponent } from "../../../mle/definitions";


class SideBarAzure {
    isCollapsed: boolean;
    favorites: Favorite[] = [];
    possibleFavorites: Favorite[] = [];

    createTooltip = "Создать (N)";
    createText = "Создать";
    browseTooltip = "Обзор (B)";
    browseText = "Обзор";
    MsPortalFxImages = new MsPortalFxImages();
    previewText = ""; //?
}

@Component({
    selector: ".fxs-sidebar",
    templateUrl: Resources.Urls.Templates.SideBar,
    encapsulation: ViewEncapsulation.None,
    styleUrls: Resources.WidgetBase,
    host: { '[class.fxs-sidebar-is-collapsed]': "isCollapsed" }
})
export class SideBarComponent extends SideBarAzure {
    constructor(bladeUiService: BladeUiService) {
        super();
        bladeUiService.sidebar = this;

        this.favorites = [
            new Favorite({ label: "Документы", uri: "#", opensExternal: false, isFavorite: false }),
            new Favorite({ label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
            new Favorite({ label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
            new Favorite({ label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        ];
        this.favorites[0].action = () => {
            //let blade = new Blade();
            //let blade2 = new Blade();
            //blade2.init = () => {
            //    blade2.contentComponent = TestBladeComponent;
            //    blade2.component.showTranslucentSpinner = true;
            //    blade2.component.loaded = true;
            //    setTimeout(() => {
            //            blade2.component.showTranslucentSpinner = false;
            //        },
            //        10000);

            //};
            //blade.init = () => {
            //    blade.contentComponent = TestBladeComponent;
            //    blade.component.showTranslucentSpinner = true;
            //    blade.component.loaded = true;
            //    setTimeout(() => {
            //            blade.component.showTranslucentSpinner = false;
            //        },
            //        10000);

            //};
            //bladeUiService.panorama.openBlade(blade, this);
            // coreService.panorama.openBlade(blade2, blade);
        }
    }

}

export class Favorite {
    constructor(fields: Partial<Favorite>) {
        Object.assign(this, fields);
    }
    label: string;
    uri: string;
    image: string;
    opensExternal: boolean;
    isFavorite: boolean;
    isFilteredOut() { };
    isPreview: boolean;
    action: Function;
}
