﻿
#include <stdio.h>
#include <cstdlib>

#include "../loader/ssloader.h"
#include "ssplayer_animedecode.h"
#include "ssplayer_matrix.h"
#include "ssplayer_render.h"


void	SsCellMapList::clear()
{
	CellMapDic.clear();
}


void	SsCellMapList::setCellMapPath(  const SsString& filepath )
{
	CellMapPath = filepath;
}

void	SsCellMapList::add(SsProject* proj)
{
	for ( size_t i = 0 ; i < proj->cellmapList.size() ; i++ )
	{
		add( proj->cellmapList[i] );
	}
}

void	SsCellMapList::set(SsProject* proj)
{
	CellMapDic.clear();
	setCellMapPath( proj->getImageBasepath() );
	add( proj );
}

void	SsCellMapList::add(SsCellMap* cellmap)
{
	SsCelMapLinker* linker = new SsCelMapLinker(cellmap , this->CellMapPath );
	CellMapDic[ cellmap->name+".ssce" ] = linker ;
}

SsCelMapLinker*	SsCellMapList::getCellMapLink( const SsString& name )
{
	SsCelMapLinker* l = CellMapDic[name];
	if ( l != 0 ) return l;

	return 0;
}


void calcUvs( SsCellValue* cellv )
{
	SsCellMap* map = cellv->cellmapl->cellMap;
	SsCell* cell = cellv->cell;

	if ( cell == 0 || map == 0)
	{
		cellv->uvs[0].x = cellv->uvs[0].y = 0;
		cellv->uvs[1].x = cellv->uvs[1].y = 0;
		cellv->uvs[2].x = cellv->uvs[2].y = 0;
		cellv->uvs[3].x = cellv->uvs[3].y = 0;
		return;
	}

	SsVector2 wh = map->pixelSize;
	// 右上に向かって＋になる
	float left = cell->pos.x / wh.x;
	float right = (cell->pos.x + cell->size.x) / wh.x;


	// LB->RB->LT->RT 順
	// 頂点をZ順にしている都合上UV値は上下逆転させている
	float top = cell->pos.y / wh.y;
	float bottom = ( cell->pos.y + cell->size.y) / wh.y;

	if (cell->rotated)
	{
		// 反時計回りに９０度回転されているため起こして描画されるようにしてやる。
		// 13
		// 02
		cellv->uvs[0].x = cellv->uvs[1].x = left;
		cellv->uvs[2].x = cellv->uvs[3].x = right;
		cellv->uvs[1].y = cellv->uvs[3].y = top;
		cellv->uvs[0].y = cellv->uvs[2].y = bottom;
	}
	else
	{
		// そのまま。頂点の順番は下記の通り
		// 01
		// 23
		cellv->uvs[0].x = cellv->uvs[2].x = left;
		cellv->uvs[1].x = cellv->uvs[3].x = right;
		cellv->uvs[0].y = cellv->uvs[1].y = top;
		cellv->uvs[2].y = cellv->uvs[3].y = bottom;
	}
}