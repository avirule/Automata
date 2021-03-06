namespace Automata.Engine.Rendering.Fonts.FreeTypePrimitives
{
	/// <summary>
	///     <para>
	///         A list of values that are used to select a specific hinting algorithm to use by the hinter. You should OR one
	///         of these values to your ‘load_flags’ when calling <see cref="Face.LoadGlyph" />.
	///     </para>
	///     <para>
	///         Note that font's native hinters may ignore the hinting algorithm you  have specified (e.g., the TrueType
	///         bytecode interpreter). You can set <see cref="LoadFlags.ForceAutohint" /> to ensure that the auto-hinter is
	///         used.
	///     </para>
	///     <para>
	///         Also note that <see cref="LoadTarget.Light" /> is an exception, in that it always implies
	///         <see cref="LoadFlags.ForceAutohint" />.
	///     </para>
	/// </summary>
	/// <remarks>
	///     <para>
	///         You should use only one of the <see cref="LoadTarget" /> values in your ‘load_flags’. They can't be ORed.
	///     </para>
	///     <para>
	///         If <see cref="LoadFlags.Render" /> is also set, the glyph is rendered in the corresponding mode (i.e., the mode
	///         which matches the used algorithm best) unless <see cref="LoadFlags.Monochrome" /> is set.
	///     </para>
	///     <para>
	///         You can use a hinting algorithm that doesn't correspond to the same rendering mode. As an example, it is
	///         possible to use the ‘light’ hinting algorithm and have the results rendered in horizontal LCD pixel mode, with
	///         code like:
	///         <code>
	/// FT_Load_Glyph( face, glyph_index,
	///          load_flags | FT_LOAD_TARGET_LIGHT );
	/// 
	/// FT_Render_Glyph( face->glyph, FT_RENDER_MODE_LCD );
	/// </code>
	///     </para>
	/// </remarks>
	public enum LoadTarget
    {
	    /// <summary>
	    ///     This corresponds to the default hinting algorithm, optimized for standard gray-level rendering. For
	    ///     monochrome output, use <see cref="LoadTarget.Mono" /> instead.
	    /// </summary>
	    /// <remarks>
	    ///     Copied from SharpFont (https://github.com/Robmaister/SharpFont)
	    /// </remarks>
	    Normal = (RenderMode.Normal & 15) << 16,

	    /// <summary>
	    ///     <para>
	    ///         A lighter hinting algorithm for non-monochrome modes. Many generated glyphs are more fuzzy but better
	    ///         resemble its original shape. A bit like rendering on Mac OS X.
	    ///     </para>
	    ///     <para>
	    ///         As a special exception, this target implies <see cref="LoadFlags.ForceAutohint" />.
	    ///     </para>
	    /// </summary>
	    Light = (RenderMode.Light & 15) << 16,

	    /// <summary>
	    ///     Strong hinting algorithm that should only be used for monochrome output. The result is probably unpleasant
	    ///     if the glyph is rendered in non-monochrome modes.
	    /// </summary>
	    Mono = (RenderMode.Mono & 15) << 16,

	    /// <summary>
	    ///     A variant of <see cref="LoadTarget.Normal" /> optimized for horizontally decimated LCD displays.
	    /// </summary>
	    LCD = (RenderMode.LCD & 15) << 16,

	    /// <summary>
	    ///     A variant of <see cref="LoadTarget.Normal" /> optimized for vertically decimated LCD displays.
	    /// </summary>
	    VerticalLCD = (RenderMode.VerticalLCD & 15) << 16
    }
}
