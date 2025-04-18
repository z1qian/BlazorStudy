﻿namespace BlazingPizza.ComponentsLibrary.Map;

public class Marker : Point
{
    public string Description { get; set; }

    public bool ShowPopup { get; set; }

    public override string ToString() => $"{Description} ({X}, {Y})";
}
