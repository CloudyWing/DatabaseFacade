namespace CloudyWing.DatabaseFacade.Tests {
    [TestFixture]
    internal class KeepConnectionRequiredExceptionTests {
        [Test]
        public void Constructor_WhenInnerExceptionIsException_ShouldKeepInnerException() {
            InvalidOperationException inner = new("Inner");

            KeepConnectionRequiredException exception = new("Outer", inner);

            using (Assert.EnterMultipleScope()) {
                Assert.That(exception.Message, Is.EqualTo("Outer"));
                Assert.That(exception.InnerException, Is.SameAs(inner));
            }
        }

        [Test]
        public void Constructor_WhenInnerExceptionIsKeepConnectionRequiredException_ShouldKeepInnerException() {
            KeepConnectionRequiredException inner = new("Inner");

            KeepConnectionRequiredException exception = new("Outer", inner);

            Assert.That(exception.InnerException, Is.SameAs(inner));
        }
    }
}
