import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
	providedIn: 'root'
})
export class AuthService {

	private currentUserSubject!: BehaviorSubject<User>;
	public currentUser!: Observable<User>;

	constructor(private http: HttpClient) {
		this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser') || "{}"));
		this.currentUser = this.currentUserSubject.asObservable();
	}

	signin(code: string) {
		var query = {
			code: code
		};
		return this.http.post(environment.baseUrl + `Auth/SignIn`, query).pipe(map(user => {
			return this.storeUser(user);
		}));
	}

	signout()
	{
		localStorage.removeItem('currentUser');
		this.currentUserSubject.next(new User());
	}

	storeUser(user: any) {
		localStorage.setItem('currentUser', JSON.stringify(user));
		this.currentUserSubject.next(user);
		return user;
	}

	getAuthorizationUrl() {
		return this.http.get(environment.baseUrl + `Auth/GetAuthorizationUrl`);
	}
}
