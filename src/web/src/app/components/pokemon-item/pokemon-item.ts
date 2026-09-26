import { Component, EventEmitter, input, Input, Output, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Pokemon } from '../../models/pokemon';
import { MatIcon } from '@angular/material/icon';


@Component({
  imports: [MatCardModule, MatButtonModule, MatIcon],
  selector: 'app-pokemon-item',
  styleUrl: './pokemon-item.scss',
  templateUrl: './pokemon-item.html',
})

export class PokemonItem {

  @Input() pokemonItem!: Pokemon;
  @Input() actionButtonText!: string;
  @Input() owned!: boolean;

  @Output() actionClicked = new EventEmitter<Pokemon>();

  onActionButtonClick(){
    this.actionClicked.emit(this.pokemonItem);
  }

}