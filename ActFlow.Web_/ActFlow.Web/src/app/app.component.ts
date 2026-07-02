import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TuiRoot } from "@taiga-ui/core";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterModule, TuiRoot],
    template: `
		<tui-root style="height:100vh"><router-outlet></router-outlet></tui-root>
    `
})
export class AppComponent {
}
