import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
    selector: 'app-fil-temporary',
    imports: [
    CommonModule
],
    template: `
        <span>temporary</span>
    `,
    host:{
        class: 'base-view'
    }
})
export class FILTemporary {
}
