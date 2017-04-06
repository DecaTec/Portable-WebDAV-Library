using System;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Represents the No-Tag list as specified in <see href="https://tools.ietf.org/html/rfc4918#section-10.4.2"/>
    /// </summary>
    public class NoTagList
    {
        /// <summary>
        /// The coded-URL of this <see cref="NoTagList"/>. <para/>
        /// See <see href="https://tools.ietf.org/html/rfc4918#section-10.4.2"/> for more information.
        /// </summary>
        public readonly CodedUrl CodedUrl;

        /// <summary>
        /// The No-Tag List prefix.
        /// </summary>
        private const char NoTagListPrefix = '(';

        /// <summary>
        /// The No-Tag List postfix.
        /// </summary>
        private const char NoTagListPostfix = ')';

        /// <summary>
        /// Constructs a No Tag List based on the <paramref name="codedUrl"/>. <para/>
        /// See <see href="https://tools.ietf.org/html/rfc4918#section-10.4.2"/> for the No Tag List definition.
        /// </summary>
        /// <param name="codedUrl">The coded-URL for this No-Tag list.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="codedUrl"/> is null.</exception>
        internal NoTagList(CodedUrl codedUrl)
        {
            CodedUrl = codedUrl ?? throw new ArgumentNullException(nameof(codedUrl));
        }

        /// <inheritdoc />
        public override string ToString() => $"{NoTagListPrefix}{CodedUrl}{NoTagListPostfix}";

        /// <summary>
        /// Tries to parse the given <paramref name="rawNoTagList"/> to a <see cref="NoTagList"/>. <para/>
        /// See <see href="https://tools.ietf.org/html/rfc4918#section-10.4.2"/> for the No-Tag List definition.
        /// </summary>
        /// <param name="rawNoTagList">The raw No-Tag List to parse into the <see cref="NoTagList"/>.</param>
        /// <param name="noTagList">The <see cref="NoTagList"/>.</param>
        /// <returns>The parsed <see cref="NoTagList"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rawNoTagList"/> is null.</exception>
        public static bool TryParse(string rawNoTagList, out NoTagList noTagList)
        {
            if (rawNoTagList == null)
                throw new ArgumentNullException(nameof(rawNoTagList));

            if (!rawNoTagList.StartsWith(NoTagListPrefix.ToString()) || !rawNoTagList.EndsWith(NoTagListPostfix.ToString()))
            {
                noTagList = null;
                return false;
            }

            var rawCodedUrl = rawNoTagList.Trim(NoTagListPrefix, NoTagListPostfix);
            if (CodedUrl.TryParse(rawCodedUrl, out var codedUrl))
            {
                noTagList = new NoTagList(codedUrl);
                return true;
            }
            noTagList = null;
            return false;
        }
    }
}
