import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Button } from "primeng/button";

@Component({
    standalone: true,
    imports: [CommonModule, Button],
    selector: 'app-footer',
    template: `<div class="layout-footer">
        ActFlow © 2026
        <a href="https://github.com/kris701/ActFlow" pButton target="_blank" rel="noopener noreferrer">
            <p-button icon="pi pi-github" label="Repo" link/>
        </a>
    </div>`,
    styles: `
        .layout-footer {
            display: flex;
            align-items: center;
            height:4rem;
            justify-content: center;
            padding: 1rem 0 1rem 0;
            gap: 0.5rem;
            border-top: 1px solid var(--surface-border);
        }
    `
})
export class AppFooter {
}
