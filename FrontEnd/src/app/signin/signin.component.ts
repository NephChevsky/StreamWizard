import { Component, inject } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
	selector: 'app-signin',
	standalone: true,
	imports: [],
	templateUrl: './signin.component.html',
	styleUrl: './signin.component.scss'
})
export class SigninComponent {
	private route = inject(ActivatedRoute);
	private router = inject(Router);
	private authService = inject(AuthService)
	constructor() {
	}

	ngOnInit() {
		var error = this.route.snapshot.queryParams['error'];
		var code = this.route.snapshot.queryParams['code'];
		if (error)
		{
			console.log("error: " + error + "\r\n" + this.route.snapshot.queryParams['error_description']);
		}
		else if (code)
		{
			this.authService.signin(code).subscribe({
				next: () => {
					this.router.navigate([""]);
				},
				error: (err) => {
					console.log(err)
				}
			});
		}
		else
		{
			this.authService.getAuthorizationUrl().subscribe({
				next: (data: any) => {
					if (data["authorizationUrl"]) {
						window.location.href = data.authorizationUrl;
					}
				},
				error: (err) => {
					console.log(err)
				}
			});
		}
	}
}
