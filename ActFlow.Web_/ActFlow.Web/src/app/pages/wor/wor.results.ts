import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
    selector: 'app-wor-res',
    imports: [
    CommonModule
],
    template: `
        <span>works</span>
    `,
    host:{
        class: 'base-view'
    }
})
export class WORResults {
}
