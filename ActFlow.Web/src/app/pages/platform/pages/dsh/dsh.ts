import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
    selector: 'app-dsh',
    imports: [
    CommonModule
],
    template: `
        <div class="card">
            Welcome to ActFlow!
        </div>
        <div class="card">
            <p>text</p>
        </div>
    `,
    host:{
        class: 'flex flex-col flex-grow'
    }
})
export class Dashboard {
}

