import { ChangeDetectorRef, Component, inject, OnInit} from '@angular/core';
import { MatButtonModule} from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatToolbarModule} from '@angular/material/toolbar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PokemonItem } from '../../components/pokemon-item/pokemon-item';
import { CollectionService } from '../../services/collection-service';
import { CollectionModel } from '../../models/collection';
import { Pokemon } from '../../models/pokemon';
import { CollectionStateService } from '../../state/collection-state.service';

@Component({
  imports: [MatToolbarModule, MatButtonModule, MatPaginatorModule, PokemonItem],
  selector: 'app-collection',
  styleUrl: './collection.scss',
  templateUrl: './collection.html',
})

export class Collection implements OnInit {
  private collectionStateService = inject(CollectionStateService);
  private collectionService = inject(CollectionService)
  private snackBar = inject(MatSnackBar)
  private cdr = inject(ChangeDetectorRef)
  collections: CollectionModel[] = [];
  private allPokemons: Pokemon[] = [];
  pokemons: Pokemon[]  = [];

  pageIndex = 0;
  pageSize = 10;
  totalItems = 0;
  actionButtonText = "Remove From Collection";
  inCollection = true;

  ngOnInit(): void {
    this.loadCollectionPokemons();  
  }

  loadCollectionPokemons() : void {
    this.collectionService
      .getCollectionPokemons().subscribe({
        next: (response) => {
          this.collections = response;
          const collection = response[0];
          if (collection) {
            this.allPokemons = collection.pokemons;
            this.totalItems = this.allPokemons.length;
            this.collectionStateService.setCollection(collection.id)
          } else {
            this.allPokemons = [];
            this.totalItems = 0;
          }
          this.updateVisiblePokemons();
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(err);
        }
      });

  }

  onActionButtonClick(event: any){
    this.collectionService.removePokemonFromCollection(this.collections[0].id, event.itemId)
    .subscribe({
      next: () => {
        this.snackBar.open('Pokemon removed from collection.', 'Close', { duration: 3000 });
        this.loadCollectionPokemons();
      },
      error: () => this.snackBar.open('Could not remove Pokemon from collection.', 'Close', { duration: 3000 })
    })    
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updateVisiblePokemons();
  }

  private updateVisiblePokemons(): void {
    const pageCount = Math.ceil(this.allPokemons.length / this.pageSize);
    this.pageIndex = Math.min(this.pageIndex, Math.max(pageCount - 1, 0));
    const start = this.pageIndex * this.pageSize;
    this.pokemons = this.allPokemons.slice(start, start + this.pageSize);
  }
}
