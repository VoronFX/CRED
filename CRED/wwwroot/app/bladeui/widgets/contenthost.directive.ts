//import { Component, ViewEncapsulation, ElementRef, Input, ViewChild, AfterViewInit, ComponentFactoryResolver } from "@angular/core";
import { Directive, ViewContainerRef } from "@angular/core";
//import { Resources } from "../../definitions";

@Directive({
    selector: "[content-host]"
})
export class ContentHostDirective {
    constructor(public viewContainerRef: ViewContainerRef) { }
}
