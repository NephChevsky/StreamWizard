import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';

@Component({
	selector: 'app-root',
	standalone: true,
	imports: [
		RouterOutlet,
		ToolbarComponent,
		MatSidenavModule,
		MatListModule,
		MatIconModule
	],
	templateUrl: './app.component.html',
	styleUrl: './app.component.scss'
})
export class AppComponent {
	title = 'FrontEnd';
}
