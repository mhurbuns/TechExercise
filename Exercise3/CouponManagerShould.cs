using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Exercise3
{
    public class CouponManagerShould
    {
        [Fact]
        public void ThrowExceptionWhenLoggerIsNull()
        {
            ILogger logger = null;
            Action action = () => new CouponManager(logger, new Mock<ICouponProvider>().Object);

            action.Should().Throw<ArgumentNullException>("", nameof(logger));
        }

        [Fact]
        public void ThrowExceptionWhenCouponProviderIsNull()
        {
            ICouponProvider couponProvider = null;
            Action action = () => new CouponManager(new Mock<ILogger>().Object, couponProvider);

            action.Should().Throw<ArgumentNullException>(nameof(couponProvider));
        }

        [Fact]
        public void ThrowExceptionWhenEvaluatorsAreNull()
        {
            IEnumerable<Func<Coupon, Guid, bool>> evaluators = null;
            CouponManager couponManager = new CouponManager(new Mock<ILogger>().Object, new Mock<ICouponProvider>().Object);

            Func<Task<bool>> func = async () => await couponManager.CanRedeemCoupon(Guid.NewGuid(), Guid.NewGuid(), evaluators);

            func.Should().Throw<ArgumentNullException>(nameof(evaluators));
        }

        [Fact]
        public void ThrowExceptionWhenCouponIsNotFound()
        {
            Guid couponId = Guid.NewGuid();
            IEnumerable<Func<Coupon, Guid, bool>> evaluators = new Mock<IEnumerable<Func<Coupon, Guid, bool>>>().Object;
            Mock<ICouponProvider> couponProvider = new Mock<ICouponProvider>();
            couponProvider.Setup(x => x.Retrieve(It.IsAny<Guid>())).ReturnsAsync((Coupon)null);

            CouponManager couponManager = new CouponManager(new Mock<ILogger>().Object, couponProvider.Object);

            Func<Task<bool>> func = async () => await couponManager.CanRedeemCoupon(couponId, Guid.NewGuid(), evaluators);
            func.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public async void ReturnsTrueWhenNoEvaluators()
        {
            Guid couponId = Guid.NewGuid();
            List<Func<Coupon, Guid, bool>> evaluators = new List<Func<Coupon, Guid, bool>>();
            Mock<ICouponProvider> couponProvider = new Mock<ICouponProvider>();
            couponProvider.Setup(x => x.Retrieve(It.IsAny<Guid>())).ReturnsAsync(new Coupon());
            CouponManager couponManager = new CouponManager(new Mock<ILogger>().Object, couponProvider.Object);
            bool result = await couponManager.CanRedeemCoupon(couponId, Guid.NewGuid(), evaluators);
            result.Should().BeTrue();
        }

        [Fact]
        public async void ReturnsTrueForOneEvaluator()
        {
            Coupon coupon = new Coupon();
            Guid userId = Guid.NewGuid();

            bool trueCoupon(Coupon c, Guid u)
            {
                return c == coupon && u == userId;
            }

            Guid couponId = Guid.NewGuid();
            Func<Coupon, Guid, bool> trueFunc = trueCoupon;
            List<Func<Coupon, Guid, bool>> evaluators = new List<Func<Coupon, Guid, bool>> { trueFunc };
            Mock<ICouponProvider> couponProvider = new Mock<ICouponProvider>();
            couponProvider.Setup(x => x.Retrieve(It.IsAny<Guid>())).ReturnsAsync(coupon);
            CouponManager couponManager = new CouponManager(new Mock<ILogger>().Object, couponProvider.Object);
            bool result = await couponManager.CanRedeemCoupon(couponId, userId, evaluators);
            result.Should().BeTrue();
        }

        [Fact]
        public async void ReturnsFalseForOneEvaluator()
        {
            Coupon coupon = new Coupon();
            Guid userId = Guid.NewGuid();

            bool trueCoupon(Coupon c, Guid u)
            {
                return c == coupon && u == userId;
            }

            Guid couponId = Guid.NewGuid();
            Func<Coupon, Guid, bool> trueFunc = trueCoupon;
            List<Func<Coupon, Guid, bool>> evaluators = new List<Func<Coupon, Guid, bool>> { trueFunc };
            Mock<ICouponProvider> couponProvider = new Mock<ICouponProvider>();
            couponProvider.Setup(x => x.Retrieve(It.IsAny<Guid>())).ReturnsAsync(coupon);
            CouponManager couponManager = new CouponManager(new Mock<ILogger>().Object, couponProvider.Object);
            bool result = await couponManager.CanRedeemCoupon(couponId, Guid.NewGuid(), evaluators);
            result.Should().BeFalse();
        }
    }
}