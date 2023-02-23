import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IContest, IEntry, IPlayer, IEntryMember } from '../shared/interfaces';


@Component({
  selector: 'app-contest-data',
  templateUrl: './contest-data.component.html'
})
export class ContestDataComponent {
  public contests: IContest[] = [];
 

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<IContest[]>(baseUrl + 'contest'  ).subscribe(result => {
      this.contests = result
    }, error => console.error(error));
  }
}

