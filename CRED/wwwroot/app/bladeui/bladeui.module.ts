import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    Blade,
    Favorite,
    ContentHostDirective,
    BladeComponent,
    PanoramaComponent,
    SettingsComponent,
    PortalComponent,
    SideBarComponent,
    TopBarComponent,
    BladeUiService,
    AzureSvgSymbolsComponent,
    ContextPaneComponent,
    SettingsPaneComponent
} from "./definitions";

@NgModule({
    imports: [CommonModule],
    declarations: [
        ContentHostDirective,
        BladeComponent,
        PanoramaComponent,
        PortalComponent,
        SideBarComponent,
        TopBarComponent,
        AzureSvgSymbolsComponent,
        SettingsComponent,
        ContextPaneComponent,
        SettingsPaneComponent
    ],
    entryComponents: [ SettingsPaneComponent ],
    exports: [ PortalComponent ],
    bootstrap: []
})
export class BladeUiModule { }

//var DefaultFavorites: Favorite[] = [
//    { label: "Документы", uri: "#", opensExternal: false, isFavorite: false },
//    { label: "Товары", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//    { label: "Пользователи", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//    { label: "Аналитика", uri: "Mr. Nice", opensExternal: false, isFavorite: false },
//];
