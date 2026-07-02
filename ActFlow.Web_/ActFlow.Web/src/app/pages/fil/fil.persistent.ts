import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
    selector: 'app-fil-persistent',
    imports: [
    CommonModule
],
    template: `
        <span>persistent</span>
    `,
    host:{
        class: 'base-view'
    }
})
export class FILPersistent {
}
