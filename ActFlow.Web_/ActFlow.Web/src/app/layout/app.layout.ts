import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TuiScrollbar } from "@taiga-ui/core";
import { TuiNavigation } from '@taiga-ui/layout';
import { AppSideBar } from "./app.sidebar";
import { AppTopBar } from "./app.topbar";

@Component({
    selector: 'app-layout',
    standalone: true,
    imports: [CommonModule, RouterModule, AppTopBar, AppSideBar, TuiNavigation, TuiScrollbar],
    template: `
    <div class="flex flex-col h-full">
      <app-topbar></app-topbar>
      <div class="flex flex-row h-full" style="overflow:hidden">
        <app-sidebar></app-sidebar>
		<main tuiNavigationMain style="border:0px">
			<tui-scrollbar class="box">
				<router-outlet></router-outlet>
			</tui-scrollbar>
		</main>
      </div>
    </div>
    `
})
export class AppLayout {
}
