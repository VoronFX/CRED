import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    Blade, Favorite, BladeComponent, PanoramaComponent,
    SideBarComponent, TopBarComponent, BladeUiService, AzureSvgSymbolsComponent
} from "./definitions";

@NgModule({
    imports: [CommonModule],
    declarations: [BladeComponent, PanoramaComponent,
        SideBarComponent, TopBarComponent, AzureSvgSymbolsComponent],
    exports: [BladeComponent, PanoramaComponent,
        SideBarComponent, TopBarComponent, AzureSvgSymbolsComponent],
    bootstrap: []
})
export class BladeUiModule { }

//var DefaultFavorites: Favorite[] = [
//    { label: "Документы", uri: "#", opensExternal: false, isFavorite: false },
//    { label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//    { label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//    { label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//];
