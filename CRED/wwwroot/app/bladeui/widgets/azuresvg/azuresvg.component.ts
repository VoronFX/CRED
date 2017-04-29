import { Component, Input } from "@angular/core";
import { Resources } from "../../definitions";


@Component({
    selector: "[azure-svg]",
    templateUrl: Resources.Urls.Templates.AzureSvg
})
export class AzureSvgComponent {
    @Input("azure-svg") svgName: string;
}