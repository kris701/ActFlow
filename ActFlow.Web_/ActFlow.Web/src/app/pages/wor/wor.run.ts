import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
    selector: 'app-wor-run',
    imports: [
    CommonModule
],
    template: `
        <span>run</span>
    `,
    host:{
        class: 'base-view'
    }
})
export class WORRun {
}
