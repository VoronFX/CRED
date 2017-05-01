import { Injectable } from '@angular/core';
import { Blade, Favorite, BladeComponent, PanoramaComponent, SideBarComponent, TopBarComponent, ContextPaneComponent, SettingsPaneComponent } from "./definitions";

@Injectable()
export class BladeUiService {

    favorites = [
        new Favorite({
            label: "New Blade", uri: "javaScript:void(0);", opensExternal: false, isFavorite: false, action: ()=> {
            this.panorama.openBlade(new Blade(), this);
        }}),
        new Favorite({ label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        new Favorite({ label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
        new Favorite({ label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false }),
    ];
    panorama: PanoramaComponent;
    topbar: TopBarComponent;
    sidebar: SideBarComponent;
    leftContextPane: ContextPaneComponent;
    rightContextPane: ContextPaneComponent;
    loading = true;
    constructor() {
        setTimeout(() => this.rightContextPane.open(SettingsPaneComponent, null), 2000);
    }

}