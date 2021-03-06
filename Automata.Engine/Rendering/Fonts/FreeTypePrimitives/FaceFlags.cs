using System;

namespace Automata.Engine.Rendering.Fonts.FreeTypePrimitives
{
	/// <summary>
	///     A list of bit flags used in the ‘face_flags’ field of the <see cref="Face" /> structure. They inform client
	///     applications of properties of the corresponding face.
	///     Copied from SharpFont (https://github.com/Robmaister/SharpFont)
	/// </summary>
	[Flags]
    public enum FaceFlags
    {
	    /// <summary>
	    ///     No style flags.
	    /// </summary>
	    None = 0x0000,

	    /// <summary>
	    ///     Indicates that the face contains outline glyphs. This doesn't prevent bitmap strikes, i.e., a face can have
	    ///     both this and and <see cref="FaceFlags.FixedSizes" /> set.
	    /// </summary>
	    Scalable = 0x0001,

	    /// <summary>
	    ///     Indicates that the face contains bitmap strikes. See also <see cref="Face.FixedSizesCount" /> and
	    ///     <see cref="FontFace.AvailableSizes" />.
	    /// </summary>
	    FixedSizes = 0x0002,

	    /// <summary>
	    ///     Indicates that the face contains fixed-width characters (like Courier, Lucido, MonoType, etc.).
	    /// </summary>
	    FixedWidth = 0x0004,

	    /// <summary>
	    ///     Indicates that the face uses the ‘sfnt’ storage scheme. For now, this means TrueType and OpenType.
	    /// </summary>
	    Sfnt = 0x0008,

	    /// <summary>
	    ///     Indicates that the face contains horizontal glyph metrics. This should be set for all common formats.
	    /// </summary>
	    Horizontal = 0x0010,

	    /// <summary>
	    ///     Indicates that the face contains vertical glyph metrics. This is only available in some formats, not all of
	    ///     them.
	    /// </summary>
	    Vertical = 0x0020,

	    /// <summary>
	    ///     Indicates that the face contains kerning information. If set, the kerning distance can be retrieved through
	    ///     the function <see cref="FontFace.GetKerning" />. Otherwise the function always return the vector (0,0). Note
	    ///     that FreeType doesn't handle kerning data from the ‘GPOS’ table (as present in some OpenType fonts).
	    /// </summary>
	    Kerning = 0x0040,

	    /// <summary>
	    ///     Indicates that the font contains multiple masters and is capable of interpolating between them. See the
	    ///     multiple-masters specific API for details.
	    /// </summary>
	    MultipleMasters = 0x0100,

	    /// <summary>
	    ///     Indicates that the font contains glyph names that can be retrieved through
	    ///     <see cref="FontFace.GetGlyphName(uint, int)" />. Note that some TrueType fonts contain broken glyph name
	    ///     tables. Use the function <see cref="FontFace.HasPSGlyphNames" /> when needed.
	    /// </summary>
	    GlyphNames = 0x0200,

	    /// <summary>
	    ///     Used internally by FreeType to indicate that a face's stream was provided by the client application and
	    ///     should not be destroyed when <see cref="FontFace.Dispose()" /> is called. Don't read or test this flag.
	    /// </summary>
	    ExternalStream = 0x0400,

	    /// <summary>
	    ///     Set if the font driver has a hinting machine of its own. For example, with TrueType fonts, it makes sense
	    ///     to use data from the SFNT ‘gasp’ table only if the native TrueType hinting engine (with the bytecode
	    ///     interpreter) is available and active.
	    /// </summary>
	    Hinter = 0x0800,

	    /// <summary>
	    ///     <para>
	    ///         Set if the font is CID-keyed. In that case, the font is not accessed by glyph indices but by CID values.
	    ///         For subsetted CID-keyed fonts this has the consequence that not all index values are a valid argument to
	    ///         <see cref="FontFace.LoadGlyph" />. Only the CID values for which corresponding glyphs in the subsetted font
	    ///         exist make <see cref="FontFace.LoadGlyph" /> return successfully; in all other cases you get an
	    ///         <see cref="FreeTypeError.InvalidArgument" /> error.
	    ///     </para>
	    ///     <para>
	    ///         Note that CID-keyed fonts which are in an SFNT wrapper don't have this flag set since the glyphs are
	    ///         accessed in the normal way (using contiguous indices); the ‘CID-ness’ isn't visible to the application.
	    ///     </para>
	    /// </summary>
	    CidKeyed = 0x1000,

	    /// <summary>
	    ///     <para>
	    ///         Set if the font is ‘tricky’, this is, it always needs the font format's native hinting engine to get a
	    ///         reasonable result. A typical example is the Chinese font ‘mingli.ttf’ which uses TrueType bytecode
	    ///         instructions to move and scale all of its subglyphs.
	    ///     </para>
	    ///     <para>
	    ///         It is not possible to autohint such fonts using <see cref="LoadFlags.ForceAutohint" />; it will also ignore
	    ///         <see cref="LoadFlags.NoHinting" />. You have to set both <see cref="LoadFlags.NoHinting" /> and
	    ///         <see cref="LoadFlags.ForceAutohint" /> to really disable hinting; however, you probably never want this
	    ///         except for demonstration purposes.
	    ///     </para>
	    ///     <para>
	    ///         Currently, there are about a dozen TrueType fonts in the list of tricky fonts; they are hard-coded in file
	    ///         ‘ttobjs.c’.
	    ///     </para>
	    /// </summary>
	    Tricky = 0x2000,

	    /// <summary>
	    ///     Set if the font has color glyph tables. To access color glyphs use <see cref="LoadFlags.Color" />.
	    /// </summary>
	    Color = 0x4000
    }
}
