import { Injectable, Inject } from '@angular/core';
import { MessageService } from './message.service';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { IContest, IEntry, IPlayer, IEntryMember } from './shared/interfaces';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  http: HttpClient | undefined;
  public contests: IContest[] = [];

  getContests(): Observable<IContest[]> {
    
    this.http?.get<IContest[]>('http://localhost:7271/contest').subscribe(result => {
      this.contests = result
    }, error => console.error(error));
    return this.contests;
  }

  constructor(private messageService: MessageService, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

  }
}
