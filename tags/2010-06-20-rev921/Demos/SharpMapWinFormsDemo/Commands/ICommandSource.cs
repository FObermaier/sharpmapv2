﻿/*
 *	This file is part of SharpMapMapViewer
 *  SharpMapMapViewer is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.ComponentModel;
using System.Windows.Forms;

namespace MapViewer.Commands
{
    public interface ICommandSource
    {
        ICommand Command { get; set; }
    }

    public interface ICommandSource<TEventArgs>
        : ICommandSource
        where TEventArgs : CommandEventArgs
    {
        new ICommand<TEventArgs> Command { get; set; }
    }

    public interface ICommandControlSource<TControl>
        : ICommandSource, IControlSource<TControl>
        where TControl : Control
    {
    }

    public interface ICommandComponentSource<TComponent>
        : ICommandSource, IComponentSource<TComponent>
        where TComponent : IComponent
    {
    }

    public interface ICommandControlSource<TControl, TEventArgs>
        : ICommandSource<TEventArgs>, ICommandControlSource<TControl>
        where TEventArgs : CommandEventArgs
        where TControl : Control
    {
    }

    public interface ICommandComponentSource<TComponent, TEventArgs>
        : ICommandSource<TEventArgs>, ICommandComponentSource<TComponent>
        where TEventArgs : CommandEventArgs
        where TComponent : IComponent
    {
    }
}