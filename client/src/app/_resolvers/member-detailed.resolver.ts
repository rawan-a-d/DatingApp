import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';

// Route resolver
// allow us to get access to the data (in this case member in member details page) before the component is constructed
// no (Cannot read property 'photoUrl' of undefined) errors
@Injectable({
	providedIn: 'root',
})
export class MemberDetailedResolver implements Resolve<Member> {
	constructor(private memberService: MembersService) {}

	resolve(route: ActivatedRouteSnapshot): Observable<Member> {
		return this.memberService.getMember(route.paramMap.get('username'));
	}
}
