
import { ModuleWithProviders } from '@angular/core';


export interface IContest {
  id?: string;
  contestID: number;
  name: string;
  percentComplete: number;
  size: number;
  fee: number;
  entries?: IEntry[];
  players?: IPlayer[];
}

export interface IEntry {
  id: number;
  entryID: number;
  name: string;
  timeRemaining: number;
  points: number;
  rank: number;
  entryMembers: IEntryMember[];
  contestID: number;


}


export interface IPlayer {
  id: number;
  salary: string;
  points: number;
  projectedPoints: number;
  position: string;
  rosterPosition: string;
  drafted: string;
  fpts: string;

}


export interface IEntryMember {
  entryID: number;
  entryMemberPlayerName: string;
  lineupSlot: number;
  position: string;
  player?: IPlayer[];

}


//import { ModuleWithProviders } from '@angular/core';

//export interface ICustomer {
//  id?: string;
//  firstName: string;
//  lastName: string;
//  email: string;
//  address: string;
//  city: string;
//  state?: IState;
//  stateId?: number;
//  zip: number;
//  gender: string;
//  orderCount?: number;
//  orders?: IOrder[];
//  orderTotal?: number;
//}

//export interface IState {
//  id: number;
//  abbreviation: string;
//  name: string;
//}

//export interface IOrder {
//  product: string;
//  price: number;
//  quantity: number;
//  orderTotal?: number;
//}

//export interface IPagedResults<T> {
//  totalRecords: number;
//  results: T;
//}

//export interface ICustomerResponse {
//  status: boolean;
//  customer: ICustomer;
